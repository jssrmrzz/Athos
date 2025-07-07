#!/bin/bash

# Navigate to the API project directory
cd src/ReviewAutomation/Api

# Create migration for multi-tenant changes
dotnet ef migrations add AddMultiTenantSupport --context ReviewDbContext

echo "Migration created successfully!"
echo "To apply the migration, run:"
echo "dotnet ef database update --context ReviewDbContext"