import type { Block } from './Block'

export interface TextReceived extends Block {
  sessionId: string
}
