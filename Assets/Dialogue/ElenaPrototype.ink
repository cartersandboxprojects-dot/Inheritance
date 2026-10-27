EXTERNAL elena_comforted()
EXTERNAL comfort_elena()

{elena_comforted():
Elena: Thank you for staying with me earlier.
-> END
}
Elena: It is hard to be here tonight.
* [Stay with Elena.]
    ~ comfort_elena()
    {elena_comforted():
    Elena: Thank you. I do not feel quite so alone.
    - else:
    Elena: We should talk another time.
    }
    -> END
