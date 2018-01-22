/// <reference path='../../jquery/jquery.js' />
/// <reference path="../../xterm/xterm.js" />
/// <reference path="../../xterm/fit.js" />
/// <reference path="../../css-element-queries/ResizeSensor.js" />

function Console(parent, welcome, sessionId) {
  Terminal.applyAddon(fit);

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

    var terminal = new Terminal({
      scrollback: 20000,
      cursorBlink: false,
      cursorStyle: 'block',
      fontFamily: 'Droid Sans Mono, Consolas, Courier New, monospace',
      fontSize: 13,
      disableStdin: true,
      focus: false,
      theme: {
        foreground: '#000',
        background: '#fff',
        cursor: '#000',
        cursorAccent: '#000',
        selection: 'rgba(0, 52, 120, 0.25)',
        brightYellow: '#c4a000'
      }
    });

    div.terminalInstance = function() {
      return terminal;
    }

    // Support buffering.
    terminal.__backlog = new Array();
    terminal.__nextOffset = 0;
    terminal.__applyText = function(text) {
      if(this.__nextOffset === text.StartOffset ||
         this.__nextOffset === 0) {
        this.__nextOffset = text.EndOffset;
        this.write(text.Text);

        return true;
      }

      return false;
    };
    terminal.__buffer = function(text) {
      $('section#' + getSessionId(), parent).addClass('delayed');

      return this.__backlog.push(text);
    };
    terminal.__applyBuffer = function() {
      if(this.__backlog.length === 0) {
        return;
      }

      var that = this;

      this.__backlog = this.__backlog
        .sort(function(a, b) {
          return a.StartOffset - b.StartOffset;
        })
        .filter(function(item) {
          return !that.__applyText(item);
        });

      if (this.__backlog.length === 0) {
        $('section#' + getSessionId(), parent).removeClass('delayed');
      }
    }

    terminal.open(div);
    terminal.cursor = true;

    // Initial fit.
    try {
      terminal.fit();
    } catch(e) { /* Fails with Jasmine for reasons beyond me. */ };
    // Fit on resize.
    new ResizeSensor(div, function() {
      terminal.fit();
    });

    return terminal;
  };

  this.text = function(text) {
    var terminal = findOrCreateTerminal();

    if (terminal.__applyText(text)) {
      terminal.__applyBuffer();
    }
    else {
      terminal.__buffer(text);
    }
  };

  this.terminate = function() {
    $('section#' + getSessionId(), parent).addClass('terminated');
  };
}
