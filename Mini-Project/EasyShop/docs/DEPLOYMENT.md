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

Every push to `master` runs the CI workflow, which builds the solution and runs the tests.

### Automatic deployment

The same workflow can publish the site after the tests pass, so a merged pull request goes live by itself.

1. In the hosting panel, open the site's **Deploy** page and note the FTP server, login and password.
2. In the GitHub repository open **Settings > Secrets and variables > Actions** and add three repository secrets: `FTP_SERVER`, `FTP_USERNAME` and `FTP_PASSWORD`.
3. On the **Variables** tab add `DEPLOY_ENABLED` with the value `true`.
4. Merge any pull request into `master`, or re-run the latest workflow. The **deploy** job puts `app_offline.htm` on the site, uploads the changed files over FTPS, then removes `app_offline.htm`.

The upload never deletes files it did not create, so `appsettings.Production.json` and the pictures admins upload (`wwwroot/uploads`) survive every deployment. Remove `DEPLOY_ENABLED` to switch automatic deployment off.

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
| Pictures disappear after a deployment | Something deleted the `wwwroot/uploads` folder on the server, for example a Visual Studio publish with "Remove additional files at destination" ticked. Leave that option off. |
