import { create } from "zustand";

interface TypingEntry {
  senderId: string;
  timestamp: number;
}

interface ChatState {
  typingUsers: Record<number, TypingEntry>;
  setTyping: (chatId: number, senderId: string) => void;
  clearTyping: (chatId: number) => void;
}

export const useChatStore = create<ChatState>()((set) => ({
  typingUsers: {},

  setTyping(chatId, senderId) {
    set((state) => ({
      typingUsers: {
        ...state.typingUsers,
        [chatId]: { senderId, timestamp: Date.now() },
      },
    }));
  },

  clearTyping(chatId) {
    set((state) => {
      const next = { ...state.typingUsers };
      delete next[chatId];
      return { typingUsers: next };
    });
  },
}));
