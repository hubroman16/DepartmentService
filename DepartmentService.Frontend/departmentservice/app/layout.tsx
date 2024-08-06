import './globals.css';
import React from 'react';

export const metadata = {
  title: 'My App',
  description: 'Welcome to my Ant Design Layout with Next.js 13',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
