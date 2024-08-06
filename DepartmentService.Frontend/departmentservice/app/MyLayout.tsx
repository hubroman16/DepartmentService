"use client"
import React, { useState, ReactNode } from 'react';
import { Layout, Menu } from 'antd';
import {
  TableOutlined,
  TeamOutlined
} from '@ant-design/icons';
import 'antd/dist/reset.css';
import './globals.css';
import EditableTable from './EditableTable';

const { Header, Sider, Content } = Layout;

interface LayoutProps {
  children: ReactNode;
}

const MyLayout: React.FC<LayoutProps> = ({ children }) => {
  const [collapsed] = useState(false);
  const [selectedMenuKey, setSelectedMenuKey] = useState('1');

  const handleMenuClick = ({ key }: { key: string }) => {
    setSelectedMenuKey(key);
  };

  const renderContent = () => {
    switch (selectedMenuKey) {
      case '1':
        return <EditableTable />;
      default:
        return <div>Продукты</div>;
    }
  };

  return (
    <Layout style={{ height: '100vh' }}>
      <Sider trigger={null} collapsible collapsed={collapsed}>
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', padding: '20px' }}>
          <TeamOutlined style={{ fontSize: '45px', color: '#ffffff' }}/>
        </div>
        <Menu theme="dark" mode="inline" defaultSelectedKeys={['1']} onClick={handleMenuClick}>
          <Menu.Item key="1" icon={<TableOutlined />}>
            Подразделения
          </Menu.Item>
        </Menu>
      </Sider>
      <Layout>
        <Header style={{ padding: 0, background: '#fff', textAlign: 'center' }}>
          <span style={{ fontSize: '24px' }}>Организация</span>
        </Header>
        <Content style={{ margin: '24px 16px', padding: 24, background: '#fff', borderRadius: '8px' }}>
          {renderContent()}
        </Content>
      </Layout>
    </Layout>
  );
};

export default MyLayout;

