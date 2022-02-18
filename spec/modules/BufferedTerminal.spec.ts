import {
  BufferedTerminal,
  Block,
  WriteBufferedResult,
} from '../../source/Web/wwwroot/scripts/modules/BufferedTerminal';
import { Terminal } from 'xterm';

describe(BufferedTerminal.name, () => {
  let terminal: BufferedTerminal;
  let writeSpy: jasmine.Spy<any>;

  beforeEach(() => {
    terminal = new BufferedTerminal();

    writeSpy = spyOn(terminal as Terminal, 'write');
  });

  describe('buffered write', () => {
    describe('start at the beginning', () => {
      const block: Block = {
        startOffset: 0,
        endOffset: 'line 1'.length,
        text: 'line 1',
      };
      let result: WriteBufferedResult;

      beforeEach(() => {
        result = terminal.writeBuffered(block);
      });

      it('writes text', () => {
        expect(writeSpy).toHaveBeenCalledWith(block.text);
      });

      it('does not buffer', () => {
        expect(result.buffering).toBeFalse();
      });
    });

    describe('start with unseen blocks', () => {
      const block: Block = {
        startOffset: 42,
        endOffset: 'line 1'.length,
        text: 'line 1',
      };
      let result: WriteBufferedResult;

      beforeEach(() => {
        result = terminal.writeBuffered(block);
      });

      it('writes text', () => {
        expect(writeSpy).toHaveBeenCalledWith(block.text);
      });

      it('does not buffer', () => {
        expect(result.buffering).toBeFalse();
      });
    });

    describe('consecutive blocks', () => {
      const blocks: Block[] = [
        {
          startOffset: 0,
          endOffset: 'line 1'.length,
          text: 'line 1',
        },
        {
          startOffset: 'line 1'.length,
          endOffset: 'line 2'.length,
          text: 'line 2',
        },
      ];
      let result: WriteBufferedResult[];

      beforeEach(() => {
        result = blocks.map(b => terminal.writeBuffered(b));
      });

      it('writes all text', () => {
        const text = blocks.map(b => [b.text]);

        expect(writeSpy.calls.allArgs()).toEqual(text);
      });

      it('does not buffer', () => {
        result.forEach(r => expect(r.buffering).toBeFalse());
      });
    });

    describe('delayed block', () => {
      const first: Block = {
        startOffset: 0,
        endOffset: 'first'.length,
        text: 'first',
      };

      const late: Block = {
        startOffset: 'first'.length,
        endOffset: 'first-late'.length,
        text: '-late',
      };

      const early: Block = {
        startOffset: 'first-late'.length,
        endOffset: 'first-late-early'.length,
        text: '-early',
      };

      let result: WriteBufferedResult;

      beforeEach(() => {
        terminal.writeBuffered(first);
        result = terminal.writeBuffered(early);
      });

      it('does not write early text', () => {
        const text = [first].map(b => [b.text]);

        expect(writeSpy.calls.allArgs()).toEqual(text);
      });

      it('buffers block', () => {
        expect(result.buffering).toBeTrue();
      });

      describe('delay resolved', () => {
        beforeEach(() => {
          result = terminal.writeBuffered(late);
        });

        it('does not buffer', () => {
          expect(result.buffering).toBeFalse();
        });

        it('writes backlog block text', () => {
          const text = [first, late, early].map(b => [b.text]);

          expect(writeSpy.calls.allArgs()).toEqual(text);
        });
      });
    });
  });
});
