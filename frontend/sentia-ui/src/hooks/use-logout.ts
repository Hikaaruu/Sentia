import { useQueryClient } from "@tanstack/react-query";
import { useAuthStore } from "@/stores/auth.store";
import { useNavigate } from "react-router-dom";
import { globalConnection } from "./use-signalr";
import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";

export function useLogout() {
  const queryClient = useQueryClient();
  const clearAuth = useAuthStore((s) => s.clearAuth);
  const navigate = useNavigate();

  return () => {
    if (globalConnection?.state === HubConnectionState.Connected) {
      globalConnection.stop();
    }
    clearAuth();
    queryClient.clear();
    navigate("/login", { replace: true });
  };
}
