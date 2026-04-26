import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function parseUtcDate(iso: string | null | undefined): Date {
  if (!iso) return new Date();
  return new Date(iso.endsWith("Z") ? iso : iso + "Z");
}
