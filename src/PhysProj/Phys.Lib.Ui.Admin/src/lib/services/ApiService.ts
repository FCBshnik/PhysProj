import { AdminApiClient } from "$lib/api/AdminApiClient";

let authToken: string | null = null;

function fetchAuth(url: RequestInfo, init?: RequestInit) {
    if (init && authToken)
        init.headers = { ...init?.headers, 'Authorization': `Bearer ${authToken}` };
    return fetch(url, init);
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
}

 let apiService = new ApiService();
 apiService.restoreToken();
 
 export { apiService as service };
 export * from "$lib/api/AdminApiClient";