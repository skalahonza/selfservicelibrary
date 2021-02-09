export function parseCode() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    return urlParams.get('code')?.replace("#/", "");
}

export function removeCode() {
    const url = window.location.href;
    if (url.includes("?code"))
        window.location.href = url.split("?")[0];
}