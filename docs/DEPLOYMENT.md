# Deploying EasyShop

This guide publishes EasyShop to MonsterASP.NET, which offers a free plan with ASP.NET Core and a SQL Server database. Any Windows host with .NET 10 and SQL Server works the same way.

## What the free plan gives you

Checked on the provider's plan page when this guide was written, so confirm it again before you rely on it:

- .NET 10 and a SQL Server database (1 GB)
- a free `runasp.net` address
- deployment from Visual Studio, FTP or a manual pull from GitHub
- no custom domain, no automatic deploy on every push, no automatic backups
- HTTPS is switched on by asking their support

The free plan is fine for a demo or a portfolio. It is not meant for real customers.

## 1. Create the site and the database

1. Create an account and add a new website. Choose .NET 10.
2. Add a SQL Server database. Note the server name, database name, user and password the panel shows you.
3. Ask support to enable HTTPS for the site. The sign-in cookie is marked `Secure`, so the login only works over HTTPS.

## 2. Configure the app on the server

Never commit real credentials. Create a file named `appsettings.Production.json` and upload it next to the published files. It is already listed in `.gitignore`.

```json
{
  "ConnectionStrings": {
    "ConnectionString": "Server=<server>;Database=<database>;User Id=<user>;Password=<password>;TrustServerCertificate=True;"
  },
  "Seed": {
    "AdminEmail": "admin@yourdomain.com",
    "AdminPassword": "<a long unique password>"
  }
}
```

On the first start the app applies the migrations, adds the demo catalog, creates the `Admin` and `Customer` roles and creates the administrator from the `Seed` section. Everything is skipped when it already exists, so restarting is safe.

## 3. Publish

From Visual Studio, use **Publish** and import the publish profile that the hosting panel lets you download. Or publish from the command line and upload the output with FTP:

```bash
dotnet publish src/EasyShop.API/EasyShop.API.csproj --configuration Release --output ./publish
```

Every push to `master` also runs the CI workflow, which builds, runs the tests and attaches the published files as a downloadable artifact. Download it from the workflow run if you prefer not to build locally.

## 4. Check that it works

1. Open `https://<your-site>.runasp.net/health`. It should answer `{"status":"ok"}`.
2. Open the home page. The catalog should be there.
3. Sign in with the administrator from `Seed`, then register a normal customer in a private window and place an order.

## Troubleshooting

| Symptom | Likely cause |
|---|---|
| HTTP 500.30 or a blank page | The connection string is wrong or the database is not reachable. Check `appsettings.Production.json`. |
| Login does nothing | HTTPS is not enabled yet, so the secure cookie is dropped. |
| `429 Too Many Requests` on login | The rate limit allows 10 attempts a minute per address. Wait a minute. |
| No administrator account | `Seed:AdminEmail` or `Seed:AdminPassword` is missing. Add them and restart the site. |
