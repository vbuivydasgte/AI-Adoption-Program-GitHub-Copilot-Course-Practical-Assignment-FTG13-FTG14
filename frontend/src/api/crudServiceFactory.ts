import axiosInstance from "./axiosConfig";

export interface CrudService<TEntity, TCreateDto, TUpdateDto> {
  getAll: () => Promise<TEntity[]>;
  getById: (id: number) => Promise<TEntity>;
  create: (payload: TCreateDto) => Promise<TEntity>;
  update: (id: number, payload: TUpdateDto) => Promise<TEntity>;
  delete: (id: number) => Promise<void>;
}

export const createCrudService = <TEntity, TCreateDto, TUpdateDto>(
  basePath: string,
): CrudService<TEntity, TCreateDto, TUpdateDto> => ({
  getAll: async (): Promise<TEntity[]> => {
    const response = await axiosInstance.get<TEntity[]>(basePath);
    return response.data;
  },

  getById: async (id: number): Promise<TEntity> => {
    const response = await axiosInstance.get<TEntity>(`${basePath}/${id}`);
    return response.data;
  },

  create: async (payload: TCreateDto): Promise<TEntity> => {
    const response = await axiosInstance.post<TEntity>(basePath, payload);
    return response.data;
  },

  update: async (id: number, payload: TUpdateDto): Promise<TEntity> => {
    const response = await axiosInstance.put<TEntity>(
      `${basePath}/${id}`,
      payload,
    );
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await axiosInstance.delete(`${basePath}/${id}`);
  },
});
