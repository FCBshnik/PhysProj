class LocalStorageService {
  public get<T>(key:string):T|undefined {
    var strValue = window.localStorage.getItem(key)
    if (strValue)
      return JSON.parse(strValue) as T
    return undefined;
  }

  public set<T>(key:string, value:T) {
    window.localStorage.setItem(key, JSON.stringify(value))
  }
}

let service = new LocalStorageService();
export { service as localStorageService };