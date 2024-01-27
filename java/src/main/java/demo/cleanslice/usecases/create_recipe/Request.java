package demo.cleanslice.usecases.create_recipe;

import lombok.Getter;

public class Request {

    @Getter
    private String url;

    @Getter
    private String title;

    @Getter
    private String name;

    @Getter
    private String author;

    public Request(String url, String title, String name, String author) {
        this.url = url;
        this.title = title;
        this.name = name;
        this.author = author;
    }
}
