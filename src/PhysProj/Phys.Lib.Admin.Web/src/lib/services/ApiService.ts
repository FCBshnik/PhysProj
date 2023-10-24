import { AdminApiClient, ErrorModel } from "$lib/api/AdminApiClient";
import { notificationsService } from "./NotificationsService";
import { goto } from '$app/navigation';

let authToken: string | null = null;

function fetchAuth(url: RequestInfo, init?: RequestInit) {
    if (init && authToken)
        init.headers = { ...init?.headers, 'Authorization': `Bearer ${authToken}` };
    return fetch(url, init).then(r => {
        console.info(r.url);
        if (!r.url.endsWith('/api/health/check')) {
          if (r.status == 500)
            notificationsService.push("Server error");
        }
        if (r.status == 401) {
            notificationsService.push("Auth token expired");
            apiService.clearToken();
            // todo: fire event and redirect from root layout
            goto('/login');
        }
        return Promise.resolve(r);
    });
}

class ApiService extends AdminApiClient {

    constructor(){
        super("", { fetch: fetchAuth });
    }

    public isAuthorized() {
        return authToken != null;
    }

    public clearToken () {
        authToken = null;
        localStorage.removeItem("token");
        console.info('removed token');
    }

    public restoreToken() {
        authToken = localStorage.getItem("token");
        if (authToken)
            console.info('loaded token');
    }

    public setToken(token: string) {
        authToken = token;
        localStorage.setItem("token", authToken);
        console.info('saved token');
    }

    public transformResult<T>(url:string, response:Response, fn:(r:Response) => Promise<T>){
        return fn(response).catch(err => {
            if (err instanceof ErrorModel)
                notificationsService.push((err as ErrorModel).message);
            return Promise.reject(err);
        });
    }
}

 let apiService = new ApiService();
 apiService.restoreToken();
 
 export { apiService as service };
 export * from "$lib/api/AdminApiClient";