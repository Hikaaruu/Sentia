import type {
  AuthResponse,
  LoginRequest,
  MeResponse,
  RegisterRequest,
} from "./types";
import { apiClient } from "./client";

export async function login(data: LoginRequest): Promise<AuthResponse> {
  const res = await apiClient.post<AuthResponse>("/api/auth/login", data);
  return res.data;
}

export async function register(data: RegisterRequest): Promise<AuthResponse> {
  const res = await apiClient.post<AuthResponse>("/api/auth/register", data);
  return res.data;
}

export async function getMe(): Promise<MeResponse> {
  const res = await apiClient.get<MeResponse>("/api/auth/me");
  return res.data;
}
