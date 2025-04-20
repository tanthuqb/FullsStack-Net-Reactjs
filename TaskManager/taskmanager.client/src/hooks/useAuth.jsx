import { useEffect, useState } from "react";

export function useAuth() {
  const [user, setUser] = useState(undefined); 

  useEffect(() => {
    (async () => {
      try {
        const res = await fetch("http://localhost:5000/api/Auth/me", {
          credentials: "include",
        });
        if (!res.ok) throw new Error("Not authenticated");
        const profile = await res.json(); 
        setUser(profile);
      } catch (error) {
        console.warn("❌ Not authenticated", error);
        setUser(null);
      }
    })();
  }, []);

  return user;
}
