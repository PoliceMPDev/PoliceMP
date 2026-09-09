[RAGS AND RICHES 1.5.4.1]

---HOW TO INSTALL---

1. Make sure you delete any leftovers, be it from the Light version or the previous paid / subscription versions.



1.1 If you previously altered the mprr_patches resource to resolve any conflicting resources:
	The content of our patches resource has not been changed, the name however has. Just use your edited patches resource and rename it to amb-lslive-main-patches and replace the shipped one with yours.



2. Drag the [rags-riches] folder to your resources.

3. To enable this resource on your server: ensure rags-and-riches
   !Do not ensure anything else manually, everything is handled for you!


---COMMON ISSUES---

Issue: I see green containers or other construction props around the scene
Cause: You have a conflicting resource that edits the same vanilla ymaps, you'll need to manually edit these in CodeWalker.
	 Another map creator edited the same area.(\amb-lslive-main-patches\stream\ymap\downtown)

Issue: The map disappears around the Mile High Club or Legion Square at certain angles
Cause: Your Legion Square resource is editing the same occlusions as us, 
	you'll need to manually remove them from the dominant resource with CodeWalker (\amb-lslive-main-patches\stream\ymap\ocl)

Issue: My teleport script does not work with the rooms
Cause: You'll need to implement an IPL loading/unloading system, otherwise you'll get poolsize errors. It's advised to use the included script instead.

Issue: The old construction site is merging with the Mile High Club
Cause: Check if you followed the install steps correctly. If yes: The console should show that amb-lslive-main-patches has started, if that's the case then another resource 
	is likely overwriting files present in the patches resource  (\amb-lslive-main-patches\stream\ymap\downtown)







