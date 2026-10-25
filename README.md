# site-demo

A small demo: Vue 3 frontend + .NET 10 minimal API.

- `web/` Vue + Vite UI -> deploy to Cloudflare Pages
- `api/` .NET 10 API (`/api/health`, `/api/hello`, `/api/projects`) -> deploy to the Oracle VM
- `deploy/` Caddy config and systemd unit for the VM

## Run locally

Terminal 1 (API, needs the .NET 10 SDK):

    cd api
    dotnet run --urls http://localhost:5000

Terminal 2 (UI):

    cd web
    npm install
    npm run dev

Open http://localhost:5173. The Vite dev server proxies `/api` to the API.

## Deploy the UI (Cloudflare Pages)

1. Push this repo to GitHub.
2. Cloudflare dashboard -> Workers & Pages -> Create -> Pages -> connect the repo.
3. Settings:
   - Root directory: `web`
   - Build command: `npm run build`
   - Build output directory: `dist`
4. Environment variable: `VITE_API_URL` = `https://your-name.duckdns.org` (your API address, with https).
5. Deploy. Note the `https://<project>.pages.dev` address.

## Deploy the API (Oracle VM, Ubuntu)

1. Open ports 80 and 443 in the VCN security list, and in the VM firewall:

       sudo iptables -I INPUT -p tcp --dport 80 -j ACCEPT
       sudo iptables -I INPUT -p tcp --dport 443 -j ACCEPT
       sudo apt install -y iptables-persistent && sudo netfilter-persistent save

2. Create a DuckDNS name pointing at the VM's public IP.
3. Install the runtime: `sudo apt install -y dotnet-runtime-10.0 aspnetcore-runtime-10.0`
   (if apt can't find it, follow Microsoft's Ubuntu install page for .NET 10).
   Arm VMs need the arm64 build, which apt selects automatically.
4. Publish and copy (on your Mac, matching the VM CPU: `linux-arm64` for Ampere, `linux-x64` for AMD):

       cd api
       dotnet publish -c Release -r linux-arm64 --self-contained false -o publish
       scp -r publish/* ubuntu@<vm-ip>:/tmp/demo-api/

   On the VM: `sudo mkdir -p /opt/demo-api && sudo cp -r /tmp/demo-api/* /opt/demo-api/`
5. Edit `deploy/demo-api.service` (set your `.pages.dev` address), then:

       sudo cp demo-api.service /etc/systemd/system/
       sudo systemctl daemon-reload
       sudo systemctl enable --now demo-api

6. Install Caddy (https://caddyserver.com/docs/install#debian-ubuntu-raspbian), put `deploy/Caddyfile`
   at `/etc/caddy/Caddyfile` with your hostname, then `sudo systemctl reload caddy`.
7. Test: `curl https://your-name.duckdns.org/api/hello`

## Notes

- CORS only allows the origins in `AllowedOrigins`. If the UI shows "Could not reach the API", check
  that your exact `.pages.dev` address is listed and the API address uses https.
- Docker alternative: `api/Dockerfile` builds an image that listens on port 5000.
