import { ShellHub } from './modules/ShellHub';

jQuery(async () => {
  await new ShellHub().setUp({
    parent: jQuery('#terminals'),
    hideOnConnection: jQuery('#welcome'),
    status: jQuery('#status'),
    url: '/shell',
    group: window.location.search,
  });
});
