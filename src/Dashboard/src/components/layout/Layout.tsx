import { Sidebar } from "./Sidebar";
import { Topbar } from "./Topbar";

interface LayoutProps {
    children: React.ReactNode;
}

export function Layout({ children }: LayoutProps) {
    return (
        <div className="flex h-screen overflow-hidden">
            <Sidebar />
            <div className="flex flex-col flex-1">
                <Topbar />
                <main className="flex-1 overflow-y-auto p-6 bg-gray-50">
                    {children}
                </main>
            </div>
        </div>
    );
}