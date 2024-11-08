import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';

export const slideAnimation = trigger('slide', [
  state(
    'left',
    style({
      left: '*',
      width: '*',
    }),
  ),
  state(
    'right',
    style({
      left: '15svw',
      width: '85svw',
    }),
  ),
  transition('left => right', [
    animate('100ms 100ms', style({ width: '85svw' })),
    animate('500ms ease-in', style({ transform: 'translateX(15svw)' })),
  ]),
  transition('right => left', [
    animate('100ms 100ms', style({ width: '*' })),
    animate('500ms ease-in', style({ transform: 'translateX(-15svw)' })),
  ]),
]);
