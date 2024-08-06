import React, { useEffect, useState } from 'react';
import { Button, Form, Input, Table, Modal, Tag, Select, Upload, message } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { getAllDepartments, createDepartment, updateDepartment, deleteDepartment, syncDepartments, Department, DepartmentCreateUpdateRequest } from './Services/departmentService';
import { sortDepartmentsByChildren, filterDepartments } from './utils';

const { Option } = Select;
const { Search } = Input;

const EditableTable: React.FC = () => {
  // Состояния для хранения данных и управления модальным окном
  const [dataSource, setDataSource] = useState<(Department & { key: string; status?: string })[]>([]);
  const [filteredDepartments, setFilteredDepartments] = useState<(Department & { key: string; status?: string })[]>([]);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [form] = Form.useForm();
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [departmentsList, setDepartmentsList] = useState<Department[]>([]);
  const [searchValue, setSearchValue] = useState<string>('');

  // Используется для первоначальной загрузки данных и периодического обновления
  useEffect(() => {
    const fetchDepartments = async () => {
      const departments = await getAllDepartments();
      const departmentsWithKeys = departments.map((dept) => ({ ...dept, key: dept.id }));
      const sortedDepartments = sortDepartmentsByChildren(departmentsWithKeys);
      setDataSource(sortedDepartments);
      setFilteredDepartments(sortedDepartments);
      setDepartmentsList(departments);
    };

    fetchDepartments();
    const intervalId = setInterval(fetchDepartments, 3500);

    return () => clearInterval(intervalId);
  }, []);

  // Обновление фильтрованных данных при изменении поискового запроса или источника данных
  useEffect(() => {
    const filtered = filterDepartments(dataSource, searchValue);
    setFilteredDepartments(filtered);
  }, [searchValue, dataSource]);

  // Открытие модального окна для добавления подразделения
  const handleAdd = () => {
    setIsModalVisible(true);
  };

  // Закрытие модального окна
  const handleCancel = () => {
    setIsModalVisible(false);
    form.resetFields();
  };

  // Подтверждение добавления нового подразделения
  const handleOk = async () => {
    try {
      const values = await form.validateFields();
      const newDepartment: DepartmentCreateUpdateRequest = {
        name: values.name,
        parentId: values.parentId || null,
        children: [],
      };
      const createdDepartment = await createDepartment(newDepartment);
      setDataSource((prevDataSource) => {
        const updatedData = [...prevDataSource, { ...createdDepartment, key: createdDepartment.id, status: 'Неизвестен' }];
        return sortDepartmentsByChildren(updatedData);
      });
      setIsModalVisible(false);
      form.resetFields();
    } catch (error) {
      console.log('Failed to add department:', error);
    }
  };

  // Удаление выбранного подразделения
  const handleDelete = async () => {
    if (selectedRowKeys.length === 1) {
      const key = selectedRowKeys[0];
      await deleteDepartment(key as string);
      setDataSource((prevDataSource) => {
        const updatedData = prevDataSource.filter((item) => item.key !== key);
        return sortDepartmentsByChildren(updatedData);
      });
      setSelectedRowKeys([]);
    }
  };

  // Сохранение изменений в подразделении
  const handleSave = async (row: Department & { key: string }) => {
    const newData = [...dataSource];
    const index = newData.findIndex((item) => row.key === item.key);
    const item = newData[index];
    newData.splice(index, 1, { ...item, ...row });
    setDataSource(sortDepartmentsByChildren(newData));

    const updateData: DepartmentCreateUpdateRequest = {
      name: row.name,
      parentId: row.parentId,
      children: row.children || [],
    };

    await updateDepartment(row.key as string, updateData);
  };

  // Синхронизация данных с использованием загруженного файла
  const handleSync = async ({ file }: any) => {
    const formData = new FormData();
    formData.append('file', file);

    try {
      await syncDepartments(formData);
      message.success('Данные успешно синхронизированы');
      const updatedDepartments = await getAllDepartments();
      const departmentsWithKeys = updatedDepartments.map((dept) => ({ ...dept, key: dept.id }));
      const sortedDepartments = sortDepartmentsByChildren(departmentsWithKeys);
      setDataSource(sortedDepartments);
      setFilteredDepartments(sortedDepartments);
    } catch (error) {
      message.error('Не удалось синхронизировать данные');
      console.error('Failed to sync departments:', error);
    }
  };

  // Определение колонок таблицы
  const columns = [
    {
      title: 'Название',
      dataIndex: 'name',
      editable: true,
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      render: (status: string) => {
        let color = 'default';
        switch (status) {
          case 'Активен':
            color = 'green';
            break;
          case 'Заблокирован':
            color = 'red';
            break;
          case 'Неизвестен':
            color = 'blue';
            break;
          default:
            color = 'default';
        }
        return <Tag color={color}>{status}</Tag>;
      },
    },
    {
      title: 'Подразделения',
      dataIndex: 'children',
      render: (children: Department[]) => (
        <ul style={{ paddingLeft: '20px' }}>
          {children.map((child) => (
            <li key={child.id}>
              {child.name}
              {child.children && child.children.length > 0 && (
                <ul>
                  {child.children.map((grandchild) => (
                    <li key={grandchild.id}>{grandchild.name}</li>
                  ))}
                </ul>
              )}
            </li>
          ))}
        </ul>
      ),
    },
  ].map((col) => ({
    ...col,
    onCell: (record: Department & { key: string }) => ({
      record,
      editable: col.editable,
      dataIndex: col.dataIndex,
      title: col.title,
      handleSave,
    }),
  }));

  // Настройка выбора строк в таблице
  const rowSelection = {
    type: 'radio' as const,
    selectedRowKeys,
    onChange: (newSelectedRowKeys: React.Key[]) => {
      setSelectedRowKeys(newSelectedRowKeys);
    },
  };

  const Dragger = Upload.Dragger;

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <Button type="primary" onClick={handleAdd}>
          Добавить подразделение
        </Button>
        <Button type="default" danger onClick={handleDelete} disabled={selectedRowKeys.length !== 1} style={{ marginLeft: 8 }}>
          Удалить подразделение
        </Button>
        <Dragger
          name="file"
          multiple={false}
          showUploadList={false}
          customRequest={handleSync}
          style={{ marginTop: 8 }}
        >
          <Button icon={<UploadOutlined />}>
            Синхронизация данных
          </Button>
        </Dragger>
      </div>
      <Search
        placeholder="Поиск по подразделениям"
        enterButton="Поиск"
        size="large"
        value={searchValue}
        onChange={(e) => setSearchValue(e.target.value)}
        style={{ marginBottom: 16 }}
      />
      <Table
        rowSelection={rowSelection}
        columns={columns}
        dataSource={filteredDepartments}
        pagination={false}
      />
      <Modal title="Добавить подразделение" visible={isModalVisible} onOk={handleOk} onCancel={handleCancel}>
        <Form form={form} layout="vertical">
          <Form.Item
            name="name"
            label="Название"
            rules={[{ required: true, message: 'Пожалуйста, введите название' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item name="parentId" label="Родительское подразделение">
            <Select>
              <Option value={null}>Нет родительского подразделения</Option>
              {departmentsList.map((department) => (
                <Option key={department.id} value={department.id}>
                  {department.name}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default EditableTable;
