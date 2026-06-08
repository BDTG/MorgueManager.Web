import { initializeApp } from "https://www.gstatic.com/firebasejs/10.11.0/firebase-app.js";
import { getAuth, signInWithPopup, GoogleAuthProvider } from "https://www.gstatic.com/firebasejs/10.11.0/firebase-auth.js";

const firebaseConfig = {
  apiKey: "AIzaSyCYRv1mJjRTa-HPvfVpZk8YqED941QyCQc",
  authDomain: "morguemanager-588fd.firebaseapp.com",
  projectId: "morguemanager-588fd",
  storageBucket: "morguemanager-588fd.firebasestorage.app",
  messagingSenderId: "740307579938",
  appId: "1:740307579938:web:b315e515463ce4322a067b",
  measurementId: "G-CEJ73C6VM0"
};
// Khởi tạo Firebase
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
const provider = new GoogleAuthProvider();

// Hàm được thiết lập ở global scope (window) để Blazor WASM có thể gọi thông qua JSInterop
window.loginWithGoogle = async () => {
    try {
        const result = await signInWithPopup(auth, provider);
        const user = result.user;
        const token = await user.getIdToken();
        
        // Trả về dữ liệu cần thiết cho C# xử lý
        return {
            success: true,
            token: token,
            email: user.email,
            displayName: user.displayName,
            photoUrl: user.photoURL
        };
    } catch (error) {
        console.error("Lỗi đăng nhập Google:", error);
        return {
            success: false,
            errorMessage: error.message
        };
    }
};
