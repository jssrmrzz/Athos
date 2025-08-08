#!/bin/bash

# Athos Development Startup Script
# Alternative to npm run dev for systems without Node.js

echo "🚀 Starting Athos Development Environment..."
echo ""

# Start backend in background
echo "📡 Starting .NET API backend..."
dotnet run --project src/ReviewAutomation/Api/Athos.ReviewAutomation.Api.csproj &
BACKEND_PID=$!

# Wait a moment for backend to start
sleep 3

# Start frontend in background  
echo "🎨 Starting React frontend..."
cd src/Dashboard
npm run dev &
FRONTEND_PID=$!
cd ../..

echo ""
echo "✅ Both services starting..."
echo "🌐 Frontend: http://localhost:5173"
echo "📡 Backend: http://localhost:7157"
echo ""
echo "Press Ctrl+C to stop both services"

# Function to cleanup background processes
cleanup() {
    echo ""
    echo "🛑 Stopping services..."
    kill $BACKEND_PID 2>/dev/null
    kill $FRONTEND_PID 2>/dev/null
    echo "✅ All services stopped"
    exit 0
}

# Set up signal trap to cleanup on exit
trap cleanup SIGINT SIGTERM

# Wait for user to press Ctrl+C
wait