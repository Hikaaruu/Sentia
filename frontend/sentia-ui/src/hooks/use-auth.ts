import { useMutation, useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { login, register, getMe } from "@/api/auth";
import type { LoginRequest, RegisterRequest } from "@/api/types";
import { useAuthStore } from "@/stores/auth.store";

export function useLogin() {
  const setAuth = useAuthStore((s) => s.setAuth);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (data: LoginRequest) => login(data),
    onSuccess(data) {
      setAuth({ userId: data.userId, username: data.username }, data.token);
      navigate("/chats", { replace: true });
    },
    onError() {
      toast.error("Invalid username or password");
    },
  });
}

export function useRegister() {
  const setAuth = useAuthStore((s) => s.setAuth);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (data: RegisterRequest) => register(data),
    onSuccess(data) {
      setAuth({ userId: data.userId, username: data.username }, data.token);
      navigate("/chats", { replace: true });
    },
    onError() {
      toast.error("Registration failed. Username may already be taken.");
    },
  });
}

export function useMe() {
  const token = useAuthStore((s) => s.token);

  return useQuery({
    queryKey: ["me"],
    queryFn: getMe,
    enabled: !!token,
    retry: false,
  });
}
