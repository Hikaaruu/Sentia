import { create } from "zustand";
import type { MeResponse } from "@/api/types";

const TOKEN_KEY = "sentia_token";

interface AuthState {
  user: MeResponse | null;
  token: string | null;
  setAuth: (user: MeResponse, token: string) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()((set) => ({
  user: null,
  token: localStorage.getItem(TOKEN_KEY),

  setAuth(user, token) {
    localStorage.setItem(TOKEN_KEY, token);
    set({ user, token });
  },

  clearAuth() {
    localStorage.removeItem(TOKEN_KEY);
    set({ user: null, token: null });
  },
}));
