import axios from "axios";

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL as string,
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("sentia_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error: unknown) => {
    if (axios.isAxiosError(error) && error.response?.status === 401) {
      localStorage.removeItem("sentia_token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);
