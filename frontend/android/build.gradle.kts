allprojects {
    repositories {
        google()
        mavenCentral()
    }
}

val newBuildDir: Directory = rootProject.layout.buildDirectory.dir("../../build").get()
rootProject.layout.buildDirectory.value(newBuildDir)

subprojects {
    val newSubprojectBuildDir: Directory = newBuildDir.dir(project.name)
    project.layout.buildDirectory.value(newSubprojectBuildDir)

    afterEvaluate {
        if (project.extensions.findByName("android") != null) {
            project.extensions.configure<com.android.build.gradle.BaseExtension>("android") {
                val currentSdkVersion = compileSdkVersion
                if (currentSdkVersion != null) {
                    val versionString = currentSdkVersion
                    val version = if (versionString.startsWith("android-")) {
                        versionString.substringAfter("android-").toIntOrNull()
                    } else {
                        versionString.toIntOrNull()
                    }
                    
                    if (version != null && version < 36) {
                        compileSdkVersion("android-36")
                    }
                }
            }
        }
    }
}

subprojects {
    project.evaluationDependsOn(":app")
}

tasks.register<Delete>("clean") {
    delete(rootProject.layout.buildDirectory)
}
