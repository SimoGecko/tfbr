# Coding Pipeline

## Developing a new feature

To be able to undisturbed developing a feature without the need to adjust the code over and over again for changes from other features, we will work with different branches for each feature. Of course, if it's only about to change 3 lines, one can directly do this in the develop-branch itself. If one works on a bundle of small features, please still create a new branch and try to give it a meaningful "bundle-name".

When developing on a feature, following steps have to be done:

1. Open the branch develop and be sure to have the latest code-base.
```
git checkout develop
git pull
```
2. Create a new branch with a short but meaningful name. Replace `<feature-name>` with the corresponding name, e.g `collision-handling`. Naming convention: all lower-case, word seperated by `-`, no special characters.
```
git checkout -b feature/<feature-name>
```
3. Work in this branch on the feature. Please commit and push even here on a regular basis to better reporoduce the changes.
```
git add .
git commit -m "<commit-message>"
git push origin feature/<feature-name>
```
4. Once the feature is finished or it is in a solid state to share with the others, one can create a pull-request.
    1. Be sure to have pushed all on your branch you want to share.
    2. Login to the git-page of the project: https://gitlab.inf.ethz.ch/PRV-GTC/gamelab-2018-team2
    3. In the menu locate "Merge Requests", or open by the link: https://gitlab.inf.ethz.ch/PRV-GTC/gamelab-2018-team2/merge_requests
    4. Create a new merge-request:
        1. Source branch: `feature/<feature-name>`
        2. Target branch: `develop`
        3. Description: Short but meaningful about what important has changed. Not to many details needed.
        4. Assignee: @aemch (please add @aemch, since he will be responsible for the code-quality)
        5. Check "Remove source branch when merge request is accepted." if you are completely finished with the feature-implementation and don't need to continue working on this branch.
        6. Finish by "Submit merge request"
    5. Once I reviewd the merge-request, the assignee either finish it or let you know if there are things wich should be changed. (be it performance-wise, naming-conventions, other things)


## Fixing a bug

Since it is in the nature of coding, we will most likely running into bugs. The process is identical as for developing a new feature, but the brancch will be named: `bugfix/<bug-name>`
The same naming-conventions hold as for the features. Sometimes it is hard to find a short but meaningful name of a bug, but still try to. Otherwise just a small missbehaviour like `bugfix/steering-always-moves-left-instead-straigth` is also fine, since everybody knows by looking at it what is beeing fixed in there.


# DoD

Once finished a new feature or fixed a but, please go through this small checklist to be sure you didn't forget anything before merging the feature.

1. All tasks for the feature are implemented. (one should create for himself some subtasks before starting to implement the feature)
2. The coding styleguides needs to be met, please refer to the coding-styleguide paragraph.
3. Comments are written where needed.
    1. Not commenting the obvious, but rather why something is done in some special manner.
    2. Comment the function- and class-interfaces with the C#-documenting style. (see https://docs.microsoft.com/en-us/dotnet/csharp/codedoc). This is helpful since hovering in the code above such documented method will imediately reveal this and it can be helpful to have a short meaningful description. We can still discuss to which extend we will document the methods, but I rather like to documentthem all, since then we can create a documentation-website, which let it look even more professional ;-)
4. Build yields no errors.
    1. Please build in `x64` mode, since the x-box has only a 64-bit architecture
5. There are no todo's in the code.
    1. Here only todo's for the current feature are included.
    2. Todo's for future work or other features are totally fine.
    3. => This tends to be a grey line, but this should avoid to move important work towards the end when we finally don't have time anymore.
6. Deployment works for the x-box
    1. If you cant try this out => no worries, but let somebody who can know so we can have a look if all works still as expected. This should reduce the time to find some x-box related bugs for some implementations and at this stage we know the changes.
    2. Most likely ask @aemch for this, since he is responsible for the deployments


# Coding styleguides

## Code formating

1. `{`-brackets are on the same line as the previous code.
    1. For methods:
        1. Good:
        ```
        public Add(int a, int b) {
        }
        ```
        2. Bad:
        ```
        public Add(int a, int b)
        {
        }
        ```
    2. For if-else statements or any loops
        1. Good:
        ```
        if (test == true) {
            // code
        } else if (test2 == true) {
            // code
        } else {
            // code
        }
        ```
        2. Bad:
        ```
        if (test == true)
        {
            // code
        }
        else if (test2 == true)
        {
            // code
        }
        else
        {
            // code
        }
        ```

## Naming conventions

1. Methods are written im upper camel case
    1. Good:
    ```
    public ThisIsAnExample(int a, int b) {}
    ```
    2. Bad:
    ```
    public thisIsAnExample(int a, int b) {}
    public thisisanexample(int a, int b) {}
    public this_is_an_example(int a, int b) {}
    ```
2. Parameters are written in camel case
    1. Good:
    ```
    public Add(int firstParameter, float secondParameter) {}
    ```
    2. Bad:
    ```
    public Add(int first_parameter, float SecondParameter) {}
    ```
3. Private attributes (careful: this is not a Property): has to start with an underscore and then camel case. Public attributes without getter-setter are not allowed.
    1. Good:
    ```
    public class GoodExample {
        private int _myPrivateAttribute;
        private Vector _position;
    }
    ```
    2. Bad:
    ```
    public class BadExample {
        private int MyPrivateAttribute;
        private Vector position;
        public float _orientation;
    }
    ```
4. Properties have to be in upper camel case. (Does not matter if the accessors are public, protected or private)
    1. Good:
    ```
    public class GoodExample {
        public int FirstProperty { get; set; }
        public int SecondProperty { get; private set; }

        // ok, but unnessecary
        private int _thirdAttribute;
        public int ThirdAttribute {
            get { return _thirdAttribute; }
            set { _thirdAttribute = value; }
        }
    }
    ```
    2. Bad:
    ```
    public class BadExample {
        public int firstProperty { get; set; }
        public int _secondProperty { get; private set; }
    }
    ```
5. Constants are as properties written always in upper camel case:
    1. Good:
    ```
    private const float LinearVelocity;
    public const float AngularVelocity;
    ```
    2. Bad:
    ```
    private const float _linearVelocity;
    public const float angularVelocity;
    ```

## Structure of the files-classes
1. Each file lies in the correct namespace
    1. Good: File "MyClass.cs" is in the dictionary "Engine/Physics"
    ```
    namespace BRS.Engine.Physics {}
    ```
    2. Bad: Same file as above
    ```
    namespace BRS.AnyThingElse {}
    ```
2. In each file there is exactly one class
    1. If there are many helper classes for something, rather put them all in the folder "Helpers" and then create there for each class a file. In this case one finds easier where which class is.