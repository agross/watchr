import { Terminal } from '@xterm/xterm'
import type { Block } from './Block'

export interface WriteBufferedResult {
  buffering: boolean
}

export class BufferedTerminal extends Terminal {
  private backlog: Block[] = []
  private nextOffset = 0

  public writeBuffered(text: Block): WriteBufferedResult {
    if (text.startOffset >= this.nextOffset) {
      this.saveToBacklog(text)
    }
    this.applyBacklog()

    return {
      buffering: this.backlog.length !== 0
    }
  }

  private saveToBacklog(block: Block) {
    this.backlog.push(block)
  }

  private applyBacklog() {
    this.backlog = this.backlog
      .sort((a, b) => a.startOffset - b.startOffset)
      .filter((item) => !this.apply(item))
  }

  private apply(block: Block): boolean {
    if (this.nextOffset === block.startOffset || this.nextOffset === 0) {
      this.nextOffset = block.endOffset

      this.write(block.text)
      return true
    }

    return false
  }
}
