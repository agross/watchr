import { describe, it, expect, beforeEach, afterEach, vi, type SpyInstance } from 'vitest'

import { BufferedTerminal, type WriteBufferedResult } from '../BufferedTerminal'
import type { Block } from '../Block'

describe(BufferedTerminal.name, () => {
  let terminal: BufferedTerminal
  let writeSpy: SpyInstance<[data: string | Uint8Array, callback?: () => void], void>

  beforeEach(() => {
    terminal = new BufferedTerminal()

    writeSpy = vi.spyOn(terminal, 'write')
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('buffered write', () => {
    describe('start at the beginning', () => {
      const block: Block = {
        startOffset: 0,
        endOffset: 'line 1'.length,
        text: 'line 1'
      }
      let result: WriteBufferedResult

      beforeEach(() => {
        result = terminal.writeBuffered(block)
      })

      it('writes text', () => {
        expect(writeSpy).toHaveBeenCalledWith(block.text)
      })

      it('does not buffer', () => {
        expect(result.buffering).toBeFalsy()
      })
    })

    describe('start with unseen blocks', () => {
      const block: Block = {
        startOffset: 42,
        endOffset: 'line 1'.length,
        text: 'line 1'
      }
      let result: WriteBufferedResult

      beforeEach(() => {
        result = terminal.writeBuffered(block)
      })

      it('writes text', () => {
        expect(writeSpy).toHaveBeenCalledWith(block.text)
      })

      it('does not buffer', () => {
        expect(result.buffering).toBeFalsy()
      })
    })

    describe('consecutive blocks', () => {
      const blocks: Block[] = [
        {
          startOffset: 0,
          endOffset: 'first line'.length,
          text: 'first line'
        },
        {
          startOffset: 'first line'.length,
          endOffset: 'second line'.length,
          text: 'second line'
        }
      ]
      let result: WriteBufferedResult[]

      beforeEach(() => {
        result = blocks.map((b) => terminal.writeBuffered(b))
      })

      it('writes all text', () => {
        const text = blocks.map((b) => [b.text])

        expect(writeSpy.mock.calls).toEqual(text)
      })

      it('does not buffer', () => {
        result.forEach((r) => expect(r.buffering).toBeFalsy())
      })
    })

    describe('delayed block', () => {
      const first: Block = {
        startOffset: 0,
        endOffset: 'first'.length,
        text: 'first'
      }

      const late: Block = {
        startOffset: 'first'.length,
        endOffset: 'first-late'.length,
        text: '-late'
      }

      const early: Block = {
        startOffset: 'first-late'.length,
        endOffset: 'first-late-early'.length,
        text: '-early'
      }

      let result: WriteBufferedResult

      describe('late blocks arrive when we are past their end offset', () => {
        beforeEach(() => {
          terminal.writeBuffered(first)
          result = terminal.writeBuffered(first)
        })

        it('ignores late blocks', () => {
          expect(writeSpy.mock.calls.length).toEqual(1)
        })

        it('does not buffer', () => {
          expect(result.buffering).toBeFalsy()
        })
      })

      describe('first then delayed', () => {
        beforeEach(() => {
          terminal.writeBuffered(first)
          result = terminal.writeBuffered(early)
        })

        it('does not write early text', () => {
          const text = [first].map((b) => [b.text])

          expect(writeSpy.mock.calls).toEqual(text)
        })

        it('buffers block', () => {
          expect(result.buffering).toBeTruthy()
        })

        describe('delay resolved', () => {
          beforeEach(() => {
            result = terminal.writeBuffered(late)
          })

          it('does not buffer', () => {
            expect(result.buffering).toBeFalsy()
          })

          it('writes backlog block text', () => {
            const text = [first, late, early].map((b) => [b.text])

            expect(writeSpy.mock.calls).toEqual(text)
          })
        })
      })
    })
  })
})
