import { Component, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { debounceTime, filter, finalize, forkJoin, map, merge, Observable, switchMap } from 'rxjs';

import { MAX_GUESTS_PER_RESERVATION, MIN_SEARCH_LENGTH, SEARCH_DEBOUNCE_MS } from 'src/app/core/constants';
import { Guest } from 'src/app/core/models/guest.model';
import { ReservationForManipulation } from 'src/app/core/models/reservation.model';
import { Room } from 'src/app/core/models/room.model';
import { GuestService } from 'src/app/core/services/guest.service';
import { ReservationService } from 'src/app/core/services/reservation.service';
import { RoomService } from 'src/app/core/services/room.service';

const GUEST_SUGGESTIONS_COUNT = 10;
const MS_PER_DAY = 24 * 60 * 60 * 1000;

function guestSelectedValidator(control: AbstractControl): ValidationErrors | null {
  return control.value && typeof control.value === 'object' ? null : { guestNotSelected: true };
}

@Component({
  selector: 'app-reservation-form',
  templateUrl: './reservation-form.component.html',
  styleUrls: ['./reservation-form.component.scss']
})
export class ReservationFormComponent implements OnInit {
  readonly maxGuests = MAX_GUESTS_PER_RESERVATION;
  readonly today = new Date();

  readonly form = this.fb.group({
    checkInDate: [null as Date | null, Validators.required],
    checkOutDate: [null as Date | null, Validators.required],
    numberOfGuests: [1, [Validators.required, Validators.min(1), Validators.max(MAX_GUESTS_PER_RESERVATION)]],
    roomId: ['', Validators.required],
    guest: [null as Guest | string | null, guestSelectedValidator],
    notes: ['', Validators.maxLength(500)]
  });

  readonly guestOptions$: Observable<Guest[]>;

  reservationId: string | null = null;
  availableRooms: Room[] = [];
  isLoadingRooms = false;
  isSaving = false;

  private currentRoom?: Room;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private reservationService: ReservationService,
    private roomService: RoomService,
    private guestService: GuestService
  ) {
    this.guestOptions$ = this.form.controls.guest.valueChanges.pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      filter((value): value is string => typeof value === 'string' && value.length >= MIN_SEARCH_LENGTH),
      switchMap(searchTerm =>
        this.guestService.getGuests({ pageNumber: 1, pageSize: GUEST_SUGGESTIONS_COUNT, searchTerm })),
      map(result => result.items)
    );

    const { checkInDate, checkOutDate, numberOfGuests } = this.form.controls;
    merge(checkInDate.valueChanges, checkOutDate.valueChanges, numberOfGuests.valueChanges).pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      takeUntilDestroyed()
    ).subscribe(() => this.loadAvailableRooms());
  }

  get isEditMode(): boolean {
    return !!this.reservationId;
  }

  get selectedRoom(): Room | undefined {
    return this.availableRooms.find(room => room.id === this.form.controls.roomId.value);
  }

  get nights(): number {
    const { checkInDate, checkOutDate } = this.form.getRawValue();
    if (!checkInDate || !checkOutDate) {
      return 0;
    }

    return Math.max(0, Math.round((checkOutDate.getTime() - checkInDate.getTime()) / MS_PER_DAY));
  }

  get estimatedPrice(): number {
    return this.nights * (this.selectedRoom?.pricePerNight ?? 0);
  }

  ngOnInit(): void {
    this.reservationId = this.route.snapshot.paramMap.get('id');
    if (this.reservationId) {
      this.loadReservation(this.reservationId);
    }
  }

  displayGuest(guest: Guest | string | null): string {
    if (guest && typeof guest === 'object') {
      return `${guest.firstName} ${guest.lastName} (${guest.documentNumber})`;
    }

    return guest ?? '';
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const reservation: ReservationForManipulation = {
      roomId: value.roomId!,
      guestId: (value.guest as Guest).id,
      checkInDate: value.checkInDate!.toISOString(),
      checkOutDate: value.checkOutDate!.toISOString(),
      numberOfGuests: value.numberOfGuests!,
      notes: value.notes || undefined
    };

    const request$: Observable<unknown> = this.reservationId
      ? this.reservationService.updateReservation(this.reservationId, reservation)
      : this.reservationService.createReservation(reservation);

    this.isSaving = true;
    request$
      .pipe(finalize(() => this.isSaving = false))
      .subscribe(() => this.router.navigate(['/reservations']));
  }

  private loadAvailableRooms(): void {
    const { checkInDate, checkOutDate, numberOfGuests } = this.form.getRawValue();
    if (!checkInDate || !checkOutDate || checkOutDate <= checkInDate) {
      this.availableRooms = [];
      return;
    }

    this.isLoadingRooms = true;
    this.roomService.getAvailableRooms(checkInDate.toISOString(), checkOutDate.toISOString(), numberOfGuests ?? 1)
      .pipe(finalize(() => this.isLoadingRooms = false))
      .subscribe(rooms => {
        this.availableRooms = this.withCurrentRoom(rooms);
        if (!this.selectedRoom) {
          this.form.controls.roomId.setValue('');
        }
      });
  }

  /** The room of the edited reservation is reserved by that reservation itself, so it has to be added manually. */
  private withCurrentRoom(rooms: Room[]): Room[] {
    if (!this.currentRoom || rooms.some(room => room.id === this.currentRoom!.id)) {
      return rooms;
    }

    return [this.currentRoom, ...rooms];
  }

  private loadReservation(id: string): void {
    this.reservationService.getReservation(id).pipe(
      switchMap(reservation => forkJoin({
        reservation: [reservation],
        room: this.roomService.getRoom(reservation.roomId),
        guest: this.guestService.getGuest(reservation.guestId)
      }))
    ).subscribe(({ reservation, room, guest }) => {
      this.currentRoom = room;
      this.form.patchValue({
        checkInDate: new Date(reservation.checkInDate),
        checkOutDate: new Date(reservation.checkOutDate),
        numberOfGuests: reservation.numberOfGuests,
        roomId: reservation.roomId,
        guest,
        notes: reservation.notes ?? ''
      });
    });
  }
}
