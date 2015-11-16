/// <reference path="../jquery-2.1.3.js" />

function Console(parent, sessionId) {
  this.sessionId = sessionId;

  var getSessionId = function() {
    return 'session-' + sessionId;
  };

  var getLineId = function(index) {
    return getSessionId() + '-line-' + index;
  };

  var findOrCreateConsole = function() {
    var console = parent.find('section#' + getSessionId());

    if (console.length) {
      return console;
    }

    console = $('<section>').attr('id', getSessionId())
      .append($('<header>').text(sessionId))
      .append($('<div>'));

    parent.append(console);
    return console;
  };

  this.block = function(lines) {
    var div = findOrCreateConsole().find('div');

    $.each(lines, function() {
      var line = $('<pre>').attr('id', getLineId(this.Index)).html(this.Html.length == 0 ? "&nbsp;" : this.Html);

      var pre = div.find('pre#' + getLineId(this.Index));
      if (pre.length) {
        pre.replaceWith(line);
      }
      else {
        div.append(line);
      }
    });

    var bottom = div[0].scrollHeight;
    div.animate({ scrollTop: bottom }, 200);
  };

  this.terminate = function() {
    $('section#' + getSessionId()).attr('class', 'terminated');
  };
}
