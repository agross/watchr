import { ConsoleHub } from './modules/console-hub';

jQuery(async () => {
  await new ConsoleHub().setUp({
    parent: jQuery('#terminals'),
    hideOnConnection: jQuery('#welcome'),
    status: jQuery('#status'),
    group: window.location.search,
  });
});
