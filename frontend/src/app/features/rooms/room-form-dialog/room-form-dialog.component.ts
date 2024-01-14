import { Component, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize, Observable } from 'rxjs';

import { Room, ROOM_STATUSES, ROOM_TYPES, RoomStatus, RoomType } from 'src/app/core/models/room.model';
import { RoomService } from 'src/app/core/services/room.service';
import { ROOM_NUMBER_PATTERN } from 'src/app/shared/validators/form.validators';

export interface RoomFormDialogData {
  room?: Room;
}

@Component({
  selector: 'app-room-form-dialog',
  templateUrl: './room-form-dialog.component.html',
  styleUrls: ['./room-form-dialog.component.scss']
})
export class RoomFormDialogComponent {
  readonly roomTypes = ROOM_TYPES;
  readonly roomStatuses = ROOM_STATUSES;
  readonly isEditMode: boolean;

  readonly form = this.fb.nonNullable.group({
    number: ['', [Validators.required, Validators.pattern(ROOM_NUMBER_PATTERN)]],
    floor: [0, [Validators.required, Validators.min(0), Validators.max(50)]],
    type: ['Single' as RoomType, Validators.required],
    capacity: [1, [Validators.required, Validators.min(1), Validators.max(10)]],
    pricePerNight: [0, [Validators.required, Validators.min(1), Validators.max(100000)]],
    status: ['Available' as RoomStatus],
    description: ['', Validators.maxLength(500)]
  });

  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private roomService: RoomService,
    private dialogRef: MatDialogRef<RoomFormDialogComponent, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: RoomFormDialogData
  ) {
    this.isEditMode = !!data.room;
    if (data.room) {
      this.form.patchValue(data.room);
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const room = this.form.getRawValue();
    const request$: Observable<unknown> = this.data.room
      ? this.roomService.updateRoom(this.data.room.id, room)
      : this.roomService.createRoom(room);

    this.isSaving = true;
    request$
      .pipe(finalize(() => this.isSaving = false))
      .subscribe(() => this.dialogRef.close(true));
  }
}
