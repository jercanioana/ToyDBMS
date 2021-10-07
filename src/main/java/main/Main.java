package main;

import org.springframework.context.annotation.AnnotationConfigApplicationContext;
import org.springframework.web.client.RestTemplate;
import org.xml.sax.SAXException;

import javax.xml.parsers.ParserConfigurationException;
import java.io.IOException;

public class Main {
    public static void main(String[] args) throws ParserConfigurationException, SAXException, IOException{

        AnnotationConfigApplicationContext context =
                new AnnotationConfigApplicationContext(
                        "main"
                );

        RestTemplate restTemplate = context.getBean(RestTemplate.class);
        Console console = new Console(restTemplate);
        console.runConsole();

    }
}