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
      width: '84svw',
    }),
  ),
  transition('left => right', [
    animate('100ms 100ms', style({ width: '84svw' })),
    animate('500ms ease-in', style({ transform: 'translateX(15svw)' })),
  ]),
  transition('right => left', [
    animate('100ms 100ms', style({ width: '*' })),
    animate('500ms ease-in', style({ transform: 'translateX(-15svw)' })),
  ]),
]);
