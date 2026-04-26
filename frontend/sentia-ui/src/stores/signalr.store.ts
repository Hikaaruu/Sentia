import { create } from "zustand";

export type SignalRStatus = "disconnected" | "connecting" | "connected";

interface SignalRState {
  status: SignalRStatus;
  setStatus: (status: SignalRStatus) => void;
}

export const useSignalRStore = create<SignalRState>()((set) => ({
  status: "disconnected",
  setStatus: (status) => set({ status }),
}));
