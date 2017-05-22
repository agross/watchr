/// <reference path="../../jquery-2.1.3.js" />
/// <reference path="../../xterm/xterm.js" />
/// <reference path="../../css-element-queries/ResizeSensor.js" />

function Console(parent, welcome, sessionId) {
  this.sessionId = sessionId;

  var getSessionId = function() {
    return 'session-' + sessionId.replace(/[\W\s]/g, '_');
  };

  function getTerminalDiv(element) {
    return element.children().last()[0];
  }

  var findOrCreateTerminal = function() {
    welcome.hide();

    var element = parent.find('section#' + getSessionId());
    if (element.length) {
      return getTerminalDiv(element).terminalInstance();
    }

    element = $('<section>').attr('id', getSessionId())
      .append($('<header>').text(sessionId.replace(/\s.*/, '')))
      .append($('<div>').addClass('term'));

    parent.append(element);
    var div = getTerminalDiv(element);

    var terminal = new Terminal({ scrollback: 20000 });
    terminal.__backlog = new Array();
    terminal.__nextOffset = 0;
    terminal.buffer = function(text) {
      return this.__backlog.push(text);
    }

    div.terminalInstance = function() {
      return terminal;
    }

    terminal.open(div);
    terminal.cursor = true;

    new ResizeSensor(div, function() {
      terminal.fit();
    });

    return terminal;
  };

  var applyText = function(terminal, text) {
    if (terminal.__nextOffset === text.StartOffset ||
        terminal.__nextOffset === 0) {
      terminal.__nextOffset = text.EndOffset;
      terminal.write(text.Text);

      return true;
    }

    return false;
  }

  var applyBuffer = function(terminal) {
    terminal.__backlog = terminal.__backlog.filter(function(element) {
      return !applyText(terminal, element);
    });
  }

  this.text = function(text) {
    var terminal = findOrCreateTerminal();

    if (applyText(terminal, text)) {
      applyBuffer(terminal);
    }
    else {
      terminal.buffer(text);
    }
  };

  this.terminate = function() {
    $('section#' + getSessionId(), parent).attr('class', 'terminated');
  };
}
