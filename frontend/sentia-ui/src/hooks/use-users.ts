import { useInfiniteQuery } from "@tanstack/react-query";
import { getUsers } from "@/api/users";

const PAGE_SIZE = 20;

export function useUsers() {
  return useInfiniteQuery({
    queryKey: ["users"],
    queryFn: ({ pageParam }) => getUsers(pageParam as number, PAGE_SIZE),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => {
      const totalPages = Math.ceil(lastPage.totalCount / PAGE_SIZE);
      if (lastPage.page < totalPages) return lastPage.page + 1;
      return undefined;
    },
  });
}
