import { createContext, useContext, useEffect, useState } from "react"

type MockApiContextType = {
    useMockApi: boolean
    toggleMockApi: () => void
}

const MockApiContext = createContext<MockApiContextType | undefined>(undefined)

export function MockApiProvider({ children }: { children: React.ReactNode }) {
    const [useMockApi, setUseMockApi] = useState<boolean>(() => {
        const stored = localStorage.getItem("useMockApi")
        return stored !== null ? JSON.parse(stored) : true // default to true
    })

    useEffect(() => {
        localStorage.setItem("useMockApi", JSON.stringify(useMockApi))
    }, [useMockApi])

    const toggleMockApi = () => setUseMockApi(prev => !prev)

    return (
        <MockApiContext.Provider value={{ useMockApi, toggleMockApi }}>
            {children}
        </MockApiContext.Provider>
    )
}

export function useMockApiContext() {
    const context = useContext(MockApiContext)
    if (!context) {
        throw new Error("useMockApiContext must be used within a MockApiProvider")
    }
    return context
}