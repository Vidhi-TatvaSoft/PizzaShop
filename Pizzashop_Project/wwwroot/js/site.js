// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggle_fnction(demoicon, demopass_type){
    let icon= document.getElementById(demoicon);
    let pass_type = document.getElementById(demopass_type);
    

    icon.classList.toggle('bi-eye-fill');
    icon.classList.toggle('bi-eye-slash-fill');

    const type = pass_type
        .getAttribute('type') === 'password' ?
        'text' : 'password';
    pass_type.setAttribute('type', type);

}