import { lazy, Suspense } from "react";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { ProtectedRoute } from "@/router/protected-route";

const LoginPage = lazy(() => import("@/pages/auth/login-page"));
const RegisterPage = lazy(() => import("@/pages/auth/register-page"));
const ChatLayout = lazy(() => import("@/pages/chat/chat-layout"));
const ChatWindow = lazy(() => import("@/pages/chat/chat-window"));
const EmptyState = lazy(() => import("@/pages/chat/empty-state"));

export default function App() {
  return (
    <BrowserRouter>
      <Suspense>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<Navigate to="/chats" replace />} />
            <Route path="/chats" element={<ChatLayout />}>
              <Route index element={<EmptyState />} />
              <Route path=":chatId" element={<ChatWindow />} />
            </Route>
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Suspense>
    </BrowserRouter>
  );
}
