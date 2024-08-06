"use client"

import React, { useState } from 'react';
import { Layout, Menu } from 'antd';
import EditableTable from './EditableTable';
import MyLayout from './MyLayout';

const { Header, Content, Sider } = Layout;

const Page: React.FC = () => {
  const [selectedMenu, setSelectedMenu] = useState<string>('1');

  const handleMenuClick = (e: any) => {
    setSelectedMenu(e.key);
  };

  return (
    <MyLayout>
      <Layout style={{ height: '100vh' }}>
        <Sider>
          <Menu
            theme="dark"
            mode="inline"
            selectedKeys={[selectedMenu]}
            onClick={handleMenuClick}
          >
            <Menu.Item key="1">Продукты</Menu.Item>
          </Menu>
        </Sider>
        <Layout>
          <Header style={{ padding: 0, background: '#fff' }}>
            <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#333', textAlign: 'center' }}>
              Организация
            </div>
          </Header>
          <Content style={{ margin: '24px 16px', padding: 24, background: '#fff', borderRadius: '8px' }}>
            {selectedMenu === '1' && <EditableTable />}{}
          </Content>
        </Layout>
      </Layout>
    </MyLayout>
  );
};

export default Page;
