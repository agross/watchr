/// <reference path='../../jquery/jquery.min.js' />
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
    terminal.applyText = function(text) {
      if(this.__nextOffset === text.StartOffset ||
        this.__nextOffset === 0) {
        this.__nextOffset = text.EndOffset;
        this.write(text.Text);

        return true;
      }

      return false;
    };
    terminal.buffer = function(text) {
      $('section#' + getSessionId(), parent).addClass('delayed');

      return this.__backlog.push(text);
    };
    terminal.applyBuffer = function() {
      var that = this;

      this.__backlog = this.__backlog.filter(function (element) {
        return !that.applyText(element);
      });

      if (this.__backlog.length === 0) {
        $('section#' + getSessionId(), parent).removeClass('delayed');
      }
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

  this.text = function(text) {
    var terminal = findOrCreateTerminal();

    if (terminal.applyText(text)) {
      terminal.applyBuffer();
    }
    else {
      terminal.buffer(text);
    }
  };

  this.terminate = function() {
    $('section#' + getSessionId(), parent).addClass('terminated');
  };
}
