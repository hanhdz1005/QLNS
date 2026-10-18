import { ref, computed } from 'vue';

export function usePagination(initialPageSize = 7) {
  const pageIndex = ref(1);
  const pageSize = ref(initialPageSize);
  const totalCount = ref(0);
  const totalPages = ref(1);

  const setPaginationData = (total: number, pages: number) => {
    totalCount.value = total;
    totalPages.value = Math.max(1, pages);
  };

  const onPageChange = (newPage: number) => {
    pageIndex.value = newPage;
  };

  const resetPage = () => {
    pageIndex.value = 1;
  };

  const hasNextPage = computed(() => pageIndex.value < totalPages.value);
  const hasPrevPage = computed(() => pageIndex.value > 1);

  return {
    pageIndex,
    pageSize,
    totalCount,
    totalPages,
    setPaginationData,
    onPageChange,
    resetPage,
    hasNextPage,
    hasPrevPage,
  };
}
