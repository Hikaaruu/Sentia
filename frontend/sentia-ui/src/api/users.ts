import type { PaginatedResponse, UserDto } from "./types";
import { apiClient } from "./client";

export async function getUsers(
  page: number,
  pageSize = 20,
): Promise<PaginatedResponse<UserDto>> {
  const res = await apiClient.get<PaginatedResponse<UserDto>>("/api/users", {
    params: { page, pageSize },
  });
  return res.data;
}
