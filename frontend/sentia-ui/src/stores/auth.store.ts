import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { MeResponse } from "@/api/types";

interface AuthState {
  user: MeResponse | null;
  token: string | null;
  setAuth: (user: MeResponse, token: string) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,

      setAuth(user, token) {
        set({ user, token });
      },

      clearAuth() {
        set({ user: null, token: null });
      },
    }),
    {
      name: "sentia-auth",
    },
  ),
);
