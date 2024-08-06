import axios from 'axios';

// Интерфейс для описания структуры объекта "Department"
export interface Department {
  id: string;
  name: string;
  parentId: string | null;
  children: Department[];
}

// Интерфейс для создания и обновления объекта "Department"
export interface DepartmentCreateUpdateRequest {
  name: string;
  parentId: string | null;
  children: Department[];
}

// Получение всех подразделений
export const getAllDepartments = async (): Promise<Department[]> => {
  const response = await axios.get('http://localhost:5229/api/Department');
  return response.data;
};

// Создание нового подразделения
export const createDepartment = async (data: DepartmentCreateUpdateRequest): Promise<Department> => {
  const response = await axios.post('http://localhost:5229/api/Department', data);
  return response.data;
};

// Обновление существующего подразделения
export const updateDepartment = async (id: string, data: DepartmentCreateUpdateRequest): Promise<void> => {
  await axios.put(`http://localhost:5229/api/Department/${id}`, data);
};

// Удаление подразделения
export const deleteDepartment = async (id: string): Promise<void> => {
  await axios.delete(`http://localhost:5229/api/Department/${id}`);
};

// Синхронизация подразделений с использованием загруженного файла
export const syncDepartments = async (data: FormData): Promise<void> => {
  await axios.post('http://localhost:5229/api/Department/sync', data, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
};
