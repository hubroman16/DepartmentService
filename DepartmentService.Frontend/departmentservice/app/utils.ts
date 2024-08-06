import { Department } from './Services/departmentService';

// Сортировка подразделений по количеству дочерних элементов
export const sortDepartmentsByChildren = (departments: (Department & { key: string; status?: string })[]): (Department & { key: string; status?: string })[] => {
  return departments
    .filter(dept => dept.parentId === null)  // Оставляем только верхнеуровневые подразделения
    .map(dept => ({
      ...dept,
      children: (dept.children || []).sort((a, b) => (b.children?.length || 0) - (a.children?.length || 0)) // Сортировка дочерних подразделений по количеству их детей
    }))
    .sort((a, b) => (b.children?.length || 0) - (a.children?.length || 0)); // Сортировка верхнеуровневых подразделений по количеству детей
};

// Фильтрация подразделений по поисковому запросу
export const filterDepartments = (departments: (Department & { key: string; status?: string })[], search: string): (Department & { key: string; status?: string })[] => {
  return departments
    .map(dept => ({
      ...dept,
      children: dept.children ? filterDepartments(dept.children.map(child => ({ ...child, key: child.id })), search) : []
    }))
    .filter(dept => dept.name.toLowerCase().includes(search.toLowerCase()) || dept.children.length > 0);
};
