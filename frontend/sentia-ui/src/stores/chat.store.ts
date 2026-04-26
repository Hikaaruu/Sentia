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

const typingTimeouts: Record<number, ReturnType<typeof setTimeout>> = {};

export const useChatStore = create<ChatState>()((set, get) => ({
  typingUsers: {},

  setTyping(chatId, senderId) {
    if (typingTimeouts[chatId]) {
      clearTimeout(typingTimeouts[chatId]);
    }

    set((state) => ({
      typingUsers: {
        ...state.typingUsers,
        [chatId]: { senderId, timestamp: Date.now() },
      },
    }));

    typingTimeouts[chatId] = setTimeout(() => {
      get().clearTyping(chatId);
      delete typingTimeouts[chatId];
    }, 3000);
  },

  clearTyping(chatId) {
    set((state) => {
      const next = { ...state.typingUsers };
      delete next[chatId];
      return { typingUsers: next };
    });
  },
}));
