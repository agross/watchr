import { Terminal } from 'xterm';

export interface Block {
  startOffset: number;
  endOffset: number;
  text: string;
}

export interface WriteBufferedResult {
  buffering: boolean;
}

export class BufferedTerminal extends Terminal {
  private _backlog: Block[] = [];
  private _nextOffset = 0;

  public writeBuffered(text: Block): WriteBufferedResult {
    this.saveToBacklog(text);
    this.applyBacklog();

    return {
      buffering: this._backlog.length !== 0,
    };
  }

  private saveToBacklog(block: Block) {
    this._backlog.push(block);
  }

  private applyBacklog() {
    this._backlog = this._backlog
      .sort((a, b) => a.startOffset - b.startOffset)
      .filter(item => !this.apply(item));
  }

  private apply(block: Block): boolean {
    if (this._nextOffset === block.startOffset || this._nextOffset === 0) {
      this._nextOffset = block.endOffset;

      this.write(block.text);
      return true;
    }

    return false;
  }
}
