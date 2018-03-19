package com.ezfun.xgame2;
import java.io.*;
import java.util.jar.*;
import java.net.*;
import java.util.*;
import android.util.Log;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

public class FileUtils
{
    public static boolean copyFile(final File toCopy, final File destFile) {
        try {
            return FileUtils.copyStream(new FileInputStream(toCopy),
                    new FileOutputStream(destFile));
        } catch (final FileNotFoundException e) {
            e.printStackTrace();
        }
        return false;
    }

    private static boolean copyFilesRecursively(final File toCopy,
                                               final File destDir) {
        assert destDir.isDirectory();

        if (!toCopy.isDirectory()) {
            return FileUtils.copyFile(toCopy, new File(destDir, toCopy.getName()));
        } else {
            final File newDestDir = new File(destDir, toCopy.getName());
            if (!newDestDir.exists() && !newDestDir.mkdir()) {
                return false;
            }
            for (final File child : toCopy.listFiles()) {
                if (!FileUtils.copyFilesRecursively(child, newDestDir)) {
                    return false;
                }
            }
        }
        return true;
    }
    public static String removeStart(String str, String remove) {
        if (isEmpty(str) || isEmpty(remove)) {
            return str;
        }
        if (str.startsWith(remove)){
            return str.substring(remove.length());
        }
        return str;
    }
    public static boolean isEmpty(CharSequence cs) {
        return cs == null || cs.length() == 0;
    }

    public static void UnZipFiles(InputStream zipFile, File destination)
    {
        byte[] buffer = new byte[1024*4];
        ZipInputStream zis = new ZipInputStream(zipFile);

        try{
            ZipEntry ze = zis.getNextEntry();

            while(ze != null){
                String fileName = ze.getName();
                File newFile = new File(destination, fileName);

                FileOutputStream fos = new FileOutputStream(newFile);
                BufferedOutputStream bufout = new BufferedOutputStream(fos);

                Log.d("OverrideActivity", "1 file name is " + fileName);

                int len;
                while ((len = zis.read(buffer)) > 0) {
                    bufout.write(buffer, 0, len);
                }

                zis.closeEntry();
                bufout.close();
                fos.close();
                ze = zis.getNextEntry();
            }

            zis.close();
        }catch(IOException ex){
            ex.printStackTrace();
        }
    }
    public static void copyJarFile(JarURLConnection jarConnection, File destination, String prefix) throws IOException {
        JarFile jarFile = jarConnection.getJarFile();
        Log.d("OverrideActivity", "dest name is " + destination.getAbsolutePath());
        FileUtils.ensureDirectoryExists(destination);
        for (final Enumeration<JarEntry> e = jarFile.entries(); e.hasMoreElements();) {
            final JarEntry entry = e.nextElement();
            if (entry.getName().startsWith(prefix))
            {
                Log.d("OverrideActivity", "entry name is " + entry.getName());
                String fileName = removeStart(entry.getName(), prefix);
                Log.d("OverrideActivity", "fileName is " + fileName);
                if (!entry.isDirectory()) {
                    InputStream entryInputStream = null;
                    try {
                        entryInputStream = jarFile.getInputStream(entry);
                        //FileUtils.copyStream(entryInputStream, new File(destination, fileName));
                        FileUtils.UnZipFiles(entryInputStream, destination);
                    } finally {
                        if (entryInputStream != null)
                            entryInputStream.close();
                    }
                }
                else
                {
                    FileUtils.ensureDirectoryExists(new File(destination, fileName));
                }
            }
        }
    }

    public static void copyResourcesRecursively(final URL originUrl, final String destPath, final String folderName) {
        try {
            final URLConnection urlConnection = originUrl.openConnection();
            File destFolder = new File(destPath, folderName);
            if (urlConnection instanceof JarURLConnection) {
                FileUtils.copyJarFile((JarURLConnection) urlConnection, destFolder, "assets/" + folderName);
            }
        } catch (final IOException e) {
            e.printStackTrace();
        }
    }

    private static boolean copyStream(final InputStream is, final File f) {
        try {
            return FileUtils.copyStream(is, new FileOutputStream(f));
        } catch (final FileNotFoundException e) {
            e.printStackTrace();
        }
        return false;
    }

    private static boolean copyStream(final InputStream is, final OutputStream os) {
        try {
            final byte[] buf = new byte[1024];

            int len = 0;
            while ((len = is.read(buf)) > 0) {
                os.write(buf, 0, len);
            }
            is.close();
            os.close();
            return true;
        } catch (final IOException e) {
            e.printStackTrace();
        }
        return false;
    }

    private static boolean ensureDirectoryExists(final File f) {
        return f.exists() || f.mkdir();
    }
}